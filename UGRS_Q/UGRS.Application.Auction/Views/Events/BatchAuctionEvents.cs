using System;
using UGRS.Core.Application.Enum.Forms;
using UGRS.Core.Auctions.Entities.Auctions;
using UGRS.Core.Auctions.Entities.Business;
using UGRS.Core.Auctions.Entities.Inventory;

namespace UGRS.Application.Auctions.Events
{
    public class LoadAuctionArgs : EventArgs
    {
        public Auction Auction { get; set; }

        public LoadAuctionArgs(Auction pObjAuction)
        {
            this.Auction = pObjAuction;
        }
    }

    public class LoadPartnerArgs : EventArgs
    {
        public Partner Partner { get; set; }

        public LoadPartnerArgs(Partner pObjPartner)
        {
            this.Partner = pObjPartner;
        }
    }

    public class LoadPartnerClassificationArgs : EventArgs
    {
        public PartnerClassification PartnerClassification { get; set; }

        public LoadPartnerClassificationArgs(PartnerClassification pObjPartnerClassification)
        {
            this.PartnerClassification = pObjPartnerClassification;
        }
    }

    public class LoadItemTypeArgs : EventArgs
    {
        public ItemType ItemType { get; set; }
        public LoadItemTypeArgs(ItemType pObjItemType)
        {
            this.ItemType = pObjItemType;
        }
    }

    public class LoadBatchArgs : EventArgs
    {
        public Batch Batch { get; set; }
        public LoadBatchArgs(Batch pObjBatch)
        {
            this.Batch = pObjBatch;
        }
    }

    public class EditBatchArgs : EventArgs
    {
        public Batch Batch { get; set; }
        public EditBatchArgs(Batch pObjBatch)
        {
            this.Batch = pObjBatch;
        }
    }

    public class ConfirmBatchArgs : EventArgs
    {
        public Batch Batch { get; set; }
        public ConfirmBatchArgs(Batch pObjBatch)
        {
            this.Batch = pObjBatch;
        }
    }

    public class CompleteBatchArgs : EventArgs
    {
        public Batch Batch { get; set; }
        public CompleteBatchArgs(Batch pObjBatch)
        {
            this.Batch = pObjBatch;
        }
    }

    public class SaveBatchArgs : EventArgs
    {
        public Batch Batch { get; set; }
        public SaveBatchArgs(Batch pObjBatch)
        {
            this.Batch = pObjBatch;
        }
    }

    public class PrintBatchArgs : EventArgs
    {
        public Batch Batch { get; set; }
        public PrintBatchArgs(Batch pObjBatch)
        {
            this.Batch = pObjBatch;
        }
    }

    public class UndoBatchArgs : EventArgs
    {
        public Batch Batch { get; set; }
        public UndoBatchArgs(Batch pObjBatch)
        {
            this.Batch = pObjBatch;
        }
    }

    public class SkipBatchArgs : EventArgs
    {
        public int BatchNumber { get; set; }
        public SkipBatchArgs(int pIntBatchNumber)
        {
            this.BatchNumber = pIntBatchNumber;
        }
    }

    public class ChangeBatchNumberArgs : EventArgs
    {
        public int BatchNumber { get; set; }
        public ChangeBatchNumberArgs(int pIntBatchNumber)
        {
            this.BatchNumber = pIntBatchNumber;
        }
    }

    public class ChangeFormModeArgs : EventArgs
    {
        public FormModeEnum FormMode { get; set; }
        public ChangeFormModeArgs(FormModeEnum pEnmFormMode)
        {
            this.FormMode = pEnmFormMode;
        }
    }

    public delegate void ConfirmBatchEventHandler(object pObjSender, ConfirmBatchArgs pObjArgs);

    public delegate void CompleteBatchEventHandler(object pObjSender, CompleteBatchArgs pObjArgs);

    public delegate void SaveBatchEventHandler(object pObjSender, SaveBatchArgs pObjArgs);

    public delegate void PrintBatchEventHandler(object pObjSender, PrintBatchArgs pObjArgs);

    public delegate void UndoBatchEventHandler(object pObjSender, UndoBatchArgs pObjArgs);

    public delegate void SkipBatchEventHandler(object pObjSender, SkipBatchArgs pObjArgs);

    public delegate void LoadAuctionEventHandler(object pObjSender, LoadAuctionArgs pObjArgs);

    public delegate void LoadPartnerEventHandler(object pObjSender, LoadPartnerArgs pObjArgs);

    public delegate void LoadPartnerClassificationEventHandler(object pObjSender, LoadPartnerClassificationArgs pObjArgs);

    public delegate void LoadItemTypeEventHandler(object pObjSender, LoadItemTypeArgs pObjArgs);

    public delegate void LoadBatchEventHandler(object pObjSender, LoadBatchArgs pObjArgs);

    public delegate void EditBatchEventHandler(object pObjSender, EditBatchArgs pObjArgs);

    public delegate void ChangeBatchNumberEventHandler(object pObjSender, ChangeBatchNumberArgs pObjArgs);

    public delegate void ChangeFormModeEventHandler(object pObjSender, ChangeFormModeArgs pObjArgs);
}
